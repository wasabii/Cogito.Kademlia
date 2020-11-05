﻿using MessagePack;

namespace Cogito.Kademlia.MessagePack.Structures
{

    [MessagePackObject]
    public class FindValueRequest : RequestBody
    {

        [Key(8)]
        public byte[] Key { get; set; }

    }

}