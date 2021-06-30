using System;
using System.Collections.Generic;

namespace CSVC {

    public class ConfigParams {
        public ILineWriter LineWriter { get; }
        
        public bool Header { get; }
        public List<int> ReadColumns { get; }

        public ConfigParams(ILineWriter lineWriter, List<int> readColumns, bool header) {
            LineWriter = lineWriter;
            ReadColumns = readColumns;
            Header = header;
        }

        public override string ToString() {
            return $"Write Mode: {LineWriter}\n" +
                   $"Header: {Header}\n" +
                   $"ReadColumns: {String.Join(",", ReadColumns)}";
        }
    }
}