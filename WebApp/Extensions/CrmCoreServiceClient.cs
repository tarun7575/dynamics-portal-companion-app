﻿using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Diagnostics;

namespace WebApp.Extensions
{
    public class CrmCoreServiceClient
    {
        private const string ServiceEndpoint = @"/xrmservices/2011/organization.svc/web?SdkClientVersion=";
        private const string AadInstance = "https://login.microsoftonline.com/";

        private OrganizationWebProxyClient _organizationWebProxyClient;
        private OrganizationServiceContext _organizationServiceContext;
        private OrganizationServiceProxy _organizationServiceProxy;

        public CrmCoreServiceClient(IOptions<DynS2SOptions> s2sOptions, IOptions<DynConnStringOptions> connStringOptions)
        {
            if (s2sOptions.Value.Validate())
            {
                // S2S not currently implemented
                throw new Exception("Dynamics S2S authentication not currently implemented");
            }

            if (!string.IsNullOrEmpty(connStringOptions?.Value.ConnString))
            {
                Trace.TraceInformation("Setting with Conn String");
                var crmServiceClient = new CrmServiceClient(connStringOptions.Value.ConnString);
                if (crmServiceClient.IsReady)
                {
                    Trace.TraceInformation("Setting ServiceProxy with CrmServiceClient");
                    _organizationServiceProxy = crmServiceClient.OrganizationServiceProxy;
                }
                else
                {
                    throw new Exception("unable to create CrmServiceClient based on connection string");
                }
            }
        }

        public OrganizationWebProxyClient WebProxyClient
        {
            get { return _organizationWebProxyClient; }
        }

        public OrganizationServiceProxy ServiceProxy
        {
            get { return _organizationServiceProxy; }
        }

        public OrganizationServiceContext ServiceContext
        {
            get
            {
                Trace.TraceInformation("Get ServiceContext");
                if (_organizationServiceContext == null)
                {
                    Trace.TraceInformation("ServiceContext is null, creating new context with OrgService");
                    _organizationServiceContext = new OrganizationServiceContext(OrgService);
                }

                return _organizationServiceContext;
            }
        }

        public IOrganizationService OrgService
        {
            get { return (IOrganizationService)ServiceProxy ?? WebProxyClient; }
        }
    }
}