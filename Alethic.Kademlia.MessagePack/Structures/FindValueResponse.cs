﻿using MessagePack;

namespace Alethic.Kademlia.MessagePack.Structures
{

    [MessagePackObject]
    public class FindValueResponse : ResponseBody
    {

        [Key(8)]
        public Node[] Peers { get; set; }

        [Key(9)]
        public bool HasValue { get; set; }

        [Key(10)]
        public ValueInfo Value { get; set; }

    }

}