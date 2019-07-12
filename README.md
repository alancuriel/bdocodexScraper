# bdocodexScraper
Windows Console application to scrape recipes and items fromm bdocodex database website to json files. Items and recipes come from the popular MMORPG Black Desert Online.

---
## SDK
- .Net core 2.2
## Console arguments
    $ recipe -alchemy
    Usage: recipe type{-cooking,alchemy}

    Scrapes recipes for the given type
    
    $ item -material
    Usage item type{-material}

    Scrapes item for given type
## Current Scraping options
- Recipes
    - Cooking
    - Alchemy
- Items
    - Materials

# Libraries
- Selenium 3.141.0 and Selenium Chrom Driver 75.0.3770.90
- HtmlAgilityPack 1.11.8
- Newtonsoft.json 12.0.2