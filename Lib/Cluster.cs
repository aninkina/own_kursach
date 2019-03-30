using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public struct Cluster
    {
        public int Length => clusters.Length;

        public int Element(int index) => clusters[index];

        int[] clusters;

        public Cluster(int[] clusters)
        {
            this.clusters = clusters ?? throw new ArgumentNullException(nameof(clusters));
        }

        public bool IsExisted(int city) => Array.IndexOf(clusters, city) < 0 ? false : true;
        
    }
}
