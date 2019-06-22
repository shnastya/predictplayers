using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictPlayers
{
    class NodeWithChild: NodeWithoutChild
    {
        public NodeWithChild children { get; set; }
    }
}
