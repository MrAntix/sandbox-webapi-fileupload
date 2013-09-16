using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandbox.WebApi.FileUpload.Web.Uploading
{
    public class DynamicJObject
    {
        JToken _root = new JObject();

        public object Get(string path, Type type)
        {
            var token = _root.SelectToken(path ?? string.Empty);
            if (token == null) return null;

            if (type == typeof(object))
            {
                return ToObject(token);
            }

            return token.ToObject(type);
        }

        public void Set(string path, object value)
        {
            SetSerialized(path, JsonConvert.SerializeObject(value));
        }

        void SetSerialized(string path, string value)
        {
            var valueToken = JToken.Parse(value);

            if (string.IsNullOrEmpty(path))
                _root = valueToken;
            else
            {
                var pathToken = _root;
                var pathItems = path.Split('.');
                foreach (var pathItem in pathItems)
                {

                    if (pathItem.Contains("["))
                    {
                        var split = pathItem.Split('[');
                        var arrayToken = pathToken.SelectToken(split[0]) as JArray;
                        if (arrayToken == null)
                        {
                            arrayToken = new JArray();
                            pathToken[split[0]] = arrayToken;
                        }

                        var index = int.Parse(split[1].TrimEnd(']'));
                        while (arrayToken.Count() <= index)
                            arrayToken.Add(new JObject());

                        if (pathItem == pathItems.Last())
                        {

                            arrayToken[index] = valueToken;
                        }
                        else
                        {
                            var subPathToken = arrayToken[index];
                            if (subPathToken == null)
                            {
                                subPathToken = new JObject();
                                arrayToken[index] = subPathToken;
                            }

                            pathToken = subPathToken;
                        }
                    }
                    else
                    {

                        if (pathItem == pathItems.Last())
                            pathToken[pathItem] = valueToken;

                        else
                        {
                            var subPathToken = pathToken[pathItem];
                            if (subPathToken == null)
                            {
                                subPathToken = new JObject();
                                pathToken[pathItem] = subPathToken;
                            }

                            pathToken = subPathToken;
                        }
                    }
                }
            }
        }

        static object ToObject(IEnumerable<JToken> token)
        {
            var value = token as JValue;
            if (value != null)
            {
                return value.Value;
            }

            var tokenObject = token as JObject;
            if (tokenObject != null)
            {
                var expando = new ExpandoObject();
                var expandoDictionary = expando as IDictionary<string, object>;

                foreach (var property in token.OfType<JProperty>())
                {
                    expandoDictionary.Add(property.Name, ToObject(property.Value));
                }

                return expando;
            }

            var items = token as JArray;
            if (items != null)
            {
                var array = new object[items.Count];

                var index = 0;

                foreach (var arrayItem in items)
                {
                    array[index] = ToObject(arrayItem);

                    index++;
                }

                return array;
            }

            throw new ArgumentException(
                string.Format("Unknown token type '{0}'", token.GetType()), "token");
        }
    }

}