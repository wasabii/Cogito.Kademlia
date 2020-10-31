﻿using MessagePack;

namespace Cogito.Kademlia.MessagePack.Structures
{

    [MessagePackObject]
    public class PingResponse : Response
    {

        [Key(0)]
        public IpEndpoint[] Endpoints { get; set; }

    }

}