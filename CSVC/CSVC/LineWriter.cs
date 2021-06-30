using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CSVC {
    public interface ILineWriter {
        
        public void Write(List<string> line, string insert);

        public void WriteHeader(List<string> line) {}

        public void CheckBounds(int size) {
            return;
        }
    }

    public class Append : ILineWriter {
        public void Write(List<string> line, string insert) {
            line.Add(insert);
        }
    }

    public abstract class MutableLineWriter : ILineWriter {
        protected readonly int WriteColumn;

        protected MutableLineWriter(int writeColumn) {
            WriteColumn = writeColumn;
        }
        
        public void CheckBounds(int size) {
            if (WriteColumn < size && WriteColumn >= 0) {
                return;
            }
            throw new IndexOutOfRangeException(
                $"Error, write column index {WriteColumn} out of bounds for CSV file with {size} columns.");
        }

        public abstract void Write(List<string> line, string insert);
    }
    
    public class Replace : MutableLineWriter {

        public Replace(int writeColumn) : base(writeColumn) {}

        public override void Write(List<string> line, string insert) {
            line[WriteColumn] = insert;
        }
    }
    
    public class Insert : MutableLineWriter, ILineWriter {
        private readonly string _columnName;

        public Insert(int writeColumn, string columnName) : base(writeColumn) {
            _columnName = columnName;
        }

        public void WriteHeader(List<string> line) {
            line.Insert(WriteColumn, _columnName);
        }
        public override void Write(List<string> line, string insert) {
            line.Insert(WriteColumn, insert);
        }
    }
}