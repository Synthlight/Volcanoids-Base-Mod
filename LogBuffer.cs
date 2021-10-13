using System.Text;
using UnityEngine;

namespace Base_Mod {
    public class LogBuffer {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private          bool          writtenTo;

        public void Write(string str) {
            stringBuilder.Append(str);
            writtenTo = true;
        }

        public void WriteLine(string str) {
            stringBuilder.AppendLine(str);
            writtenTo = true;
        }

        public void Flush() {
            if (!writtenTo) return;
            Debug.Log(stringBuilder.ToString());
            stringBuilder.Clear();
        }
    }
}