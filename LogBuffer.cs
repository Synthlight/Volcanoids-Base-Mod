using System.Text;
using UnityEngine;

namespace Base_Mod {
    public class LogBuffer {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        public void Write(string str) {
            stringBuilder.Append(str);
        }

        public void WriteLine(string str) {
            stringBuilder.AppendLine(str);
        }

        public void Flush() {
            Debug.Log(stringBuilder.ToString());
            stringBuilder.Clear();
        }
    }
}