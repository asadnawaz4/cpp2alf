using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPP2ALF_Transfomer.TransformationEngine
{
    class customDictionary
    {
        private List<Type> _dictionary = new List<Type>();
        public customDictionary()
        {
            _dictionary = new List<Type>
            {
                new Type ("int", "Integer"),
                new Type ("double", "Real" ),
                new Type ("void", "" ),
                new Type ("string", "String" ),
                new Type ("char", "String" ),
                new Type ("bool", "Boolean" ),
                new Type ("float", "Real")
            };
        }

        public void Add(string key, string item, bool lClass = false)
        {
            _dictionary.Add(new Type(key, item,lClass));
        }

        public Type Get(string key)
        {
            if (_dictionary.Where(x => x.cppName == key).Count() > 0)
                return _dictionary.First(x => x.cppName == key);
            else
                return new Type(key, key);
        }

        public bool Contains(string key, bool headType = false)
        {
            if(headType)
            {
                if (_dictionary.Where(x => x.cppName == key && x.localClass).Count() > 0)
                    return true;
            }
            else if (_dictionary.Where(x => x.cppName == key).Count() > 0)
                return true;
            return false;
        }

        public class Type
        {
            public string cppName { get; set; }
            public string alfName { get; set; }
            public bool localClass { get; set; }
            public Type()
            {
                cppName = "";
                alfName = "";
                localClass = false;
            }

            public Type(string cName, string aName, bool lClass=false)
            {
                cppName = cName;
                alfName = aName;
                localClass = lClass;
            }
        }
    }
}
