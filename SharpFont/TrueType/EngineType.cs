﻿#region MIT License
/*Copyright (c) 2012-2013 Robert Rouhani <robert.rouhani@gmail.com>

SharpFont based on Tao.FreeType, Copyright (c) 2003-2007 Tao Framework Team

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
#endregion

namespace SharpFont.TrueType
{
    /// <summary>
    /// A list of values describing which kind of TrueType bytecode engine is implemented in a given
    /// <see cref="Library"/> instance. It is used by the <see cref="Library.GetTrueTypeEngineType"/> function.
    /// </summary>
    public enum EngineType
    {
        /// <summary>
        /// The library doesn't implement any kind of bytecode interpreter.
        /// </summary>
        None = 0,

        /// <summary><para>
        /// The library implements a bytecode interpreter that doesn't support the patented operations of the TrueType
        /// virtual machine.
        /// </para><para>
        /// Its main use is to load certain Asian fonts which position and scale glyph components with bytecode
        /// instructions. It produces bad output for most other fonts.
        /// </para></summary>
        Unpatented,

        /// <summary>
        /// The library implements a bytecode interpreter that covers the full instruction set of the TrueType virtual
        /// machine (this was governed by patents until May 2010, hence the name).
        /// </summary>
        Patented
    }
}
