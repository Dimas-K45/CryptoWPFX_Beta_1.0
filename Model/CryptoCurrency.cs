﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWPFX.Model
{
    public class CryptoCurrency
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        [JsonProperty("total_volume")]
        public decimal Volume { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("current_price")]
        public decimal Price { get; set; }
    }
}
