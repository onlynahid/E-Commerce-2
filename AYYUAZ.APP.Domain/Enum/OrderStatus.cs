using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AYYUAZ.APP.Domain.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Processed = 0,
        Accepted =  1 ,
        Rejected = 2
    }
}
