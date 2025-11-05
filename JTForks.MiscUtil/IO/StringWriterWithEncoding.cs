// <copyright file="StringWriterWithEncoding.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    /// A simple class derived from StringWriter, but which allows
    /// the user to select which Encoding is used. This is most
    /// likely to be used with XmlTextWriter, which uses the Encoding
    /// property to determine which encoding to specify in the XML.
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        /// <summary>
        /// The encoding to return in the Encoding property.
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// Initializes a new instance of the StringWriterWithEncoding class
        /// with the specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding to report.</param>
        public StringWriterWithEncoding(Encoding encoding)
        {
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        /// Initializes a new instance of the StringWriter class with the
        /// specified format control and encoding.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider object that controls formatting.</param>
        /// <param name="encoding">The encoding to report.</param>
        public StringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding)
            : base(formatProvider)
        {
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        /// Initializes a new instance of the StringWriter class that writes to the
        /// specified StringBuilder, and reports the specified encoding.
        /// </summary>
        /// <param name="sb">The StringBuilder to write to. </param>
        /// <param name="encoding">The encoding to report.</param>
        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
            : base(sb, CultureInfo.InvariantCulture)
        {
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        /// Initializes a new instance of the StringWriter class that writes to the specified
        /// StringBuilder, has the specified format provider, and reports the specified encoding.
        /// </summary>
        /// <param name="sb">The StringBuilder to write to. </param>
        /// <param name="formatProvider">An IFormatProvider object that controls formatting.</param>
        /// <param name="encoding">The encoding to report.</param>
        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding)
            : base(sb, formatProvider)
        {
            this.encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        /// <summary>
        /// Gets the Encoding in which the output is written.
        /// </summary>
        public override Encoding Encoding => this.encoding;

    }
}
