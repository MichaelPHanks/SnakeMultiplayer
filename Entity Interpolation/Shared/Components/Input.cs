﻿
namespace Shared.Components
{
    public class Input : Component
    {
        public enum Type : UInt16
        {
            RotateUp,
            RotateLeft,
            RotateRight,
            RotateDown,
            RotateMouse,
            RotateTopRight,
            RotateTopLeft,
            RotateBottomLeft,
            RotateBottomRight,
            
        }

        public Input(List<Type> inputs)
        {
            this.inputs = inputs;
        }

        public List<Type> inputs { get; private set; }
    }
}
