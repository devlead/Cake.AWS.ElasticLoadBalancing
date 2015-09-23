﻿#region Using Statements
    using System;
    using System.Collections.Generic;

    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Diagnostics;
    using Cake.Core.Annotations;

    using Amazon.ElasticLoadBalancing;
    using Amazon.ElasticLoadBalancing.Model;
#endregion



namespace Cake.AWS.ElasticLoadBalancing
{
    /// <summary>
    /// Provides a high level utility for managing transfers to and from Amazon S3.
    /// It makes extensive use of Amazon S3 multipart uploads to achieve enhanced throughput, 
    /// performance, and reliability. When uploading large files by specifying file paths 
    /// instead of a stream, TransferUtility uses multiple threads to upload multiple parts of 
    /// a single upload at once. When dealing with large content sizes and high bandwidth, 
    /// this can increase throughput significantly.
    /// </summary>
    public class LoadBalancingManager : ILoadBalancingManager
    {
        #region Fields (2)
            private readonly ICakeEnvironment _Environment;
            private readonly ICakeLog _Log;
        #endregion





        #region Constructor (1)
            /// <summary>
            /// Initializes a new instance of the <see cref="LoadBalancingManager" /> class.
            /// </summary>
            /// <param name="environment">The environment.</param>
            /// <param name="log">The log.</param>
            public LoadBalancingManager(ICakeEnvironment environment, ICakeLog log)
            {
                if (environment == null)
                {
                    throw new ArgumentNullException("environment");
                }
                if (log == null)
                {
                    throw new ArgumentNullException("log");
                }

                _Environment = environment;
                _Log = log;
            }
        #endregion





        #region Functions (3)
            private AmazonElasticLoadBalancingClient CreateClient(LoadBalancingSettings settings)
            {
                if (settings == null)
                {
                    throw new ArgumentNullException("settings");
                }
                if (String.IsNullOrEmpty(settings.AccessKey))
                {
                    throw new ArgumentNullException("settings.AccessKey");
                }
                if (String.IsNullOrEmpty(settings.SecretKey))
                {
                    throw new ArgumentNullException("settings.SecretKey");
                }

                return new AmazonElasticLoadBalancingClient(settings.AccessKey, settings.SecretKey, settings.Region);
            }



            /// <summary>
            /// Adds new instances to the load balancer.
            /// Once the instance is registered, it starts receiving traffic and requests from the load balancer. 
            /// Any instance that is not in any of the Availability Zones registered for the load balancer will be moved to the OutOfService state. 
            /// It will move to the InService state when the Availability Zone is added to the load balancer.
            /// </summary>
            /// <param name="loadBalancer">The name associated with the load balancer.</param>
            /// <param name="instances">A list of instance IDs that should be registered with the load balancer.</param>
            /// <param name="settings">The <see cref="LoadBalancingSettings"/> used during the request to AWS.</param>
            public void RegisterInstances(string loadBalancer, IList<string> instances, LoadBalancingSettings settings)
            {
                AmazonElasticLoadBalancingClient client = this.CreateClient(settings);
                RegisterInstancesWithLoadBalancerRequest request = new RegisterInstancesWithLoadBalancerRequest();

                request.LoadBalancerName = loadBalancer;

                foreach (string instance in instances)
                {
                    request.Instances.Add(new Instance(instance));
                }

                client.RegisterInstancesWithLoadBalancer(request);
            }

            /// <summary>
            /// Removes instances from the load balancer. Once the instance is deregistered, it will stop receiving traffic from the load balancer. 
            /// </summary>
            /// <param name="loadBalancer">The name associated with the load balancer.</param>
            /// <param name="instances">A list of instance IDs that should be deregistered with the load balancer.</param>
            /// <param name="settings">The <see cref="LoadBalancingSettings"/> used during the request to AWS.</param>
            public void DeregisterInstances(string loadBalancer, IList<string> instances, LoadBalancingSettings settings)
            {
                AmazonElasticLoadBalancingClient client = this.CreateClient(settings);
                DeregisterInstancesFromLoadBalancerRequest request = new DeregisterInstancesFromLoadBalancerRequest();

                request.LoadBalancerName = loadBalancer;

                foreach (string instance in instances)
                {
                    request.Instances.Add(new Instance(instance));
                }

                client.DeregisterInstancesFromLoadBalancer(request);
            }
        #endregion
    }
}