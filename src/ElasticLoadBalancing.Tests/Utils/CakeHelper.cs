﻿#region Using Statements
    using System;
    using System.IO;
    using System.Collections.Generic;

    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Diagnostics;
    using Cake.AWS.ElasticLoadBalancing;

    using NSubstitute;
#endregion



namespace Cake.AWS.ElasticLoadBalancing.Tests
{
    internal static class CakeHelper
    {
        #region Functions (3)
            public static ICakeEnvironment CreateEnvironment()
            {
                var environment = Substitute.For<ICakeEnvironment>();
                environment.WorkingDirectory = Directory.GetCurrentDirectory();

                return environment;
            }



            public static ILoadBalancingManager CreateTransferManager()
            {
                return new LoadBalancingManager(CakeHelper.CreateEnvironment(), new DebugLog());
            }
        #endregion
    }
}
