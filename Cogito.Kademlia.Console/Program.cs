﻿using System;
using System.Net;
using System.Threading.Tasks;

using Autofac;

using Cogito.Autofac;
using Cogito.Kademlia.Network;
using Cogito.Kademlia.Protocols.Protobuf;
using Cogito.Kademlia.Protocols.Udp;
using Cogito.Serilog;

using Microsoft.Extensions.Logging;

namespace Cogito.Kademlia.Console
{

    public static class Program
    {

        [RegisterAs(typeof(ILoggerConfigurator))]
        public class LoggerConfigurator : ILoggerConfigurator
        {

            public global::Serilog.LoggerConfiguration Apply(global::Serilog.LoggerConfiguration configuration)
            {
                return configuration.MinimumLevel.Verbose();
            }

        }

        public static async Task Main(string[] args)
        {
            var bld = new ContainerBuilder();
            bld.RegisterAllAssemblyModules();
            var cnt = bld.Build();

            var log = cnt.Resolve<ILogger>();
            var enc = new KProtobufMessageEncoder<KNodeId32>();
            var dec = new KProtobufMessageDecoder<KNodeId32>();
            var rtr = new KFixedRoutingTable<KNodeId32, KPeerData<KNodeId32>>(KNodeId<KNodeId32>.Create(), new KPeerData<KNodeId32>(), logger: log);
            var kad = new KEngine<KNodeId32, KPeerData<KNodeId32>>(rtr, logger: log);
            var udp = new KUdpProtocol<KNodeId32, KPeerData<KNodeId32>>(2848441, kad, enc, dec, 0, log);
            var mcd = new KUdpMulticastDiscovery<KNodeId32, KPeerData<KNodeId32>>(2848441, kad, udp, enc, dec, new KIpEndpoint(new KIp4Address(IPAddress.Parse("224.168.100.2")), 1283), log);
            await udp.StartAsync();
            await mcd.StartAsync();

            try
            {
                await mcd.ConnectAsync();
                await rtr.RefreshAsync();
            }
            catch (Exception e)
            {

            }

            System.Console.WriteLine("Started...");

            var cont = true;
            while (cont)
            {
                switch (System.Console.ReadLine())
                {
                    case "exit":
                        cont = false;
                        break;
                    case "show":
                        foreach (var node in rtr)
                        {
                            System.Console.WriteLine("{0}", node.Key);
                            foreach (var ep in node.Value.Endpoints)
                                System.Console.WriteLine("    {0}", ep);
                        }
                        break;
                }
            }

            await mcd.StopAsync();
            await udp.StopAsync();
            System.Console.WriteLine("Stopped...");
            System.Console.ReadLine();
        }

    }

}