using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;

public static class JsonTo
{
    public static string ToJson(DataTable dt)
    {
        return JsonConvert.SerializeObject(dt);
    }
    public static string ToJson_Time(DataTable dt)
    {
        Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
        timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        return JsonConvert.SerializeObject(dt, timeConverter);
    }
}
