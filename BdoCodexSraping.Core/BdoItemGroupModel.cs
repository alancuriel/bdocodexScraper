using System.Collections.Generic;

namespace BdoCodexSraping.Core
{
    public class BdoItemGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<BdoItemModel> Items { get; set;}
    }
}