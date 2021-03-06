// ***********************************************************************
// Assembly         : ACBr.Net.DFe.Core
// Author           : RFTD
// Created          : 10-16-2016
//
// Last Modified By : RFTD
// Last Modified On : 10-16-2016
// ***********************************************************************
// <copyright file="DFeICMSCst.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2016 Grupo ACBr.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using ACBr.Net.DFe.Core.Attributes;

namespace ACBr.Net.DFe.Core.Common
{
	public enum DFeICMSCst
	{
		[DFeEnum("00")]
		Cst00,

		[DFeEnum("10")]
		Cst10,

		[DFeEnum("20")]
		Cst20,

		[DFeEnum("30")]
		Cst30,

		[DFeEnum("40")]
		Cst40,

		[DFeEnum("41")]
		Cst41,

		[DFeEnum("45")]
		Cst45,

		[DFeEnum("50")]
		Cst50,

		[DFeEnum("51")]
		Cst51,

		[DFeEnum("60")]
		Cst60,

		[DFeEnum("70")]
		Cst70,

		[DFeEnum("80")]
		Cst80,

		[DFeEnum("81")]
		Cst81,

		[DFeEnum("90")]
		Cst90,

		[DFeEnum("10")]
		CstPart10,

		[DFeEnum("90")]
		CstPart90,

		[DFeEnum("41")]
		CstRep41,

		[DFeEnum("90")]
		CstICMSOutraUF,

		[DFeEnum("SN")]
		CstICMSSN
	}
}