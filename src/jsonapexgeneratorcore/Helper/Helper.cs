using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Helper {
    //JSON to Variable Conversion
    //Handle Reserved Words
    public static List<String> reservedWords = new List<String>() { 
            "abstract", "activate", "and", "any", "array", "as", "asc", "autonomous", 
			"begin", "bigdecimal", "blob", "boolean", "break", "bulk", "by", "byte", 
			"case", "cast", "catch", "char", "class", "collect", "commit", "const", "continue", "currency", 
			"date", "datetime", "decimal", "default", "delete", "desc", "do", "double", 
			"else", "end", "enum", "exception", "exit", "export", "extends", 
			"false", "final", "finally", "float", "for", "from", 
			"global", "goto", "group", 
			"having", "hint", 
			"if", "implements", "import", "in", "inner", "insert", "instanceof", "int", "integer", "interface", "into", 
			"join", 
			"like", "limit", "list", "long", "loop", 
			"map", "merge", 
			"new", "not", "null", "nulls", "number", 
			"object", "of", "on", "or", "outer", "override", 
			"package", "parallel", "pragma", "private", "protected", "public", 
			"retrieve", "return", "rollback", 
			"select", "set", "short", "sObject", "sort", "static", "string", "super", "switch", "synchronized", "system", 
			"testmethod", "then", "this", "throw", "time", "transaction", "trigger", "true", "try", 
			"undelete", "update", "upsert", "using", 
			"virtual", "void", 
			"webservice", "when", "where", "while"};
    //Handle Sub Branches
    public static List<String> jsonToClass(String jsonStr) {
        List<String> lstVars = new List<String>();

        //Parse JSON
        using(JsonDocument parsedJSON = JsonDocument.Parse(jsonStr)) {
            JsonElement root = parsedJSON.RootElement;

            foreach(JsonProperty property in root.EnumerateObject()) {
                lstVars.Add(parseVarType(property));
            }
        }
        lstVars.Remove("");
        return lstVars;
    }
    
    //This will keep the Mapping between JSONType and Apex Primitives
    private static String parseVarType (JsonProperty property) {
        String apexType = "";
        String propertyName = property.Name;
        if (reservedWords.Contains(propertyName)) {
            propertyName = "_" + propertyName;
        }

        
        if (property.GetType() == typeof(String)) {
            apexType = "String " + propertyName + " { get; set; }";
        }
        if (property.GetType() == typeof(Int32)) {
            apexType = "Integer " + propertyName + " { get; set; }";
        }
        if (property.GetType() == typeof(Object)) {
            //property.
        }
        
        return apexType;
    }

    //Create Templates here
}