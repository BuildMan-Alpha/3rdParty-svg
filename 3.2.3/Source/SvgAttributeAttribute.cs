﻿using System;
using System.Collections.Generic;

namespace Svg
{
    /// <summary>
    /// Specifies the SVG attribute name of the associated property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event)]
    public class SvgAttributeAttribute : Attribute
    {
        public const string XLinkPrefix = "xlink";
        public const string XLinkNamespace = "http://www.w3.org/1999/xlink";
        public const string XmlPrefix = "xml";
        public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        public static readonly List<KeyValuePair<string, string>> Namespaces = new List<KeyValuePair<string, string>>()
                                                                            {
                                                                                new KeyValuePair<string, string>(XLinkPrefix, XLinkNamespace),
                                                                                new KeyValuePair<string, string>(XmlPrefix, XmlNamespace)
                                                                            };

        public override bool Equals(object obj)
        {
            if (!(obj is SvgAttributeAttribute))
                return false;

            var indicator = (SvgAttributeAttribute)obj;

            // Always match if either value is string.Empty (wildcard)
            if (indicator.Name == string.Empty)
                return false;

            return string.Equals(Name, indicator.Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the name of the SVG attribute.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the namespace of the SVG attribute.
        /// </summary>
        public string NameSpace { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgAttributeAttribute"/> class.
        /// </summary>
        internal SvgAttributeAttribute()
            : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgAttributeAttribute"/> class with the specified attribute name.
        /// </summary>
        /// <param name="name">The name of the SVG attribute.</param>
        internal SvgAttributeAttribute(string name)
            : this(name, SvgNamespace.UriString) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgAttributeAttribute"/> class with the specified SVG attribute name and namespace.
        /// </summary>
        /// <param name="name">The name of the SVG attribute.</param>
        /// <param name="nameSpace">The namespace of the SVG attribute (e.g. http://www.w3.org/2000/svg).</param>
        public SvgAttributeAttribute(string name, string nameSpace)
        {
            Name = name;
            NameSpace = nameSpace;
        }
    }
}
