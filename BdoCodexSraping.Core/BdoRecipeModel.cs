﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BdoCodexSraping.Core
{
    public class BdoRecipeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public ItemGrade Grade { get; set; }
        public RecipeType Type { get; set; }
        public string SkillLevel { get; set; }
        public int Exp { get; set; }
        public List<KeyValuePair<string,int>> ItemMaterials { get; set; }
        public List<string> CraftingResults { get; set; }
    }
}
