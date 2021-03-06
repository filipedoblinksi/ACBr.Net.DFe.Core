// ***********************************************************************
// Assembly         : ACBr.Net.DFe.Core
// Author           : RFTD
// Created          : 05-04-2016
//
// Last Modified By : RFTD
// Last Modified On : 05-04-2016
// ***********************************************************************
// <copyright file="SerializerOptions.cs" company="ACBr.Net">
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

using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ACBr.Net.DFe.Core.Serializer
{
	/// <summary>
	/// Class SerializerOptions. This class cannot be inherited.
	/// </summary>
	public class SerializerOptions
	{
		#region Constantes

		/// <summary>
		/// The er r_ ms g_ maior
		/// </summary>
		internal const string ErrMsgMaior = "Tamanho maior que o m�ximo permitido";

		/// <summary>
		/// The er r_ ms g_ menor
		/// </summary>
		internal const string ErrMsgMenor = "Tamanho menor que o m�nimo permitido";

		/// <summary>
		/// The er r_ ms g_ vazio
		/// </summary>
		internal const string ErrMsgVazio = "Nenhum valor informado";

		/// <summary>
		/// The er r_ ms g_ invalido
		/// </summary>
		internal const string ErrMsgInvalido = "Conte�do inv�lido";

		/// <summary>
		/// The er r_ ms g_ maxim o_ decimais
		/// </summary>
		internal const string ErrMsgMaximoDecimais = "Numero m�ximo de casas decimais permitidas";

		/// <summary>
		/// The er r_ ms g_ maio r_ maximo
		/// </summary>
		internal const string ErrMsgMaiorMaximo = "N�mero de ocorr�ncias maior que o m�ximo permitido - M�ximo ";

		/// <summary>
		/// The er r_ ms g_ fina l_ meno r_ inicial
		/// </summary>
		internal const string ErrMsgFinalMenorInicial = "O numero final n�o pode ser menor que o inicial";

		/// <summary>
		/// The er r_ ms g_ arquiv o_ na o_ encontrado
		/// </summary>
		internal const string ErrMsgArquivoNaoEncontrado = "Arquivo n�o encontrado";

		/// <summary>
		/// The er r_ ms g_ soment e_ um
		/// </summary>
		internal const string ErrMsgSomenteUm = "Somente um campo deve ser preenchido";

		/// <summary>
		/// The er r_ ms g_ meno r_ minimo
		/// </summary>
		internal const string ErrMsgMenorMinimo = "N�mero de ocorr�ncias menor que o m�nimo permitido - M�nimo ";

		/// <summary>
		/// The ds c_ CNPJ
		/// </summary>
		internal const string DscCnpj = "CNPJ(MF)";

		/// <summary>
		/// The ds c_ CPF
		/// </summary>
		internal const string DscCpf = "CPF";

		#endregion Constantes

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DFeSerializer" /> class.
		/// </summary>
		internal SerializerOptions()
		{
			ErrosAlertas = new BindingList<string>();
			FormatoAlerta = "TAG:%TAG% ID:%ID%/%TAG%(%DESCRICAO%) - %MSG%.";
			RemoverAcentos = false;
			FormatarXml = true;
			AssinarXML = false;
			Encoder = Encoding.UTF8;
		}

		#endregion Constructors

		#region Propriedades

		/// <summary>
		/// Indica se � para retirar acentos do XML ou n�o.
		/// </summary>
		/// <value><c>true</c> if [retirar acentos]; otherwise, <c>false</c>.</value>
		public bool RemoverAcentos { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SerializerOptions"/> is identar.
		/// </summary>
		/// <value><c>true</c> if identar; otherwise, <c>false</c>.</value>
		public bool FormatarXml { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [omitir declaracao].
		/// </summary>
		/// <value><c>true</c> if [omitir declaracao]; otherwise, <c>false</c>.</value>
		public bool OmitirDeclaracao { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [assinar XML].
		/// </summary>
		/// <value><c>true</c> if [assinar XML]; otherwise, <c>false</c>.</value>
		public bool AssinarXML { get; set; }

		/// <summary>
		/// Gets or sets the sign URI.
		/// </summary>
		/// <value>The sign URI.</value>
		public string SignUri { get; set; }

		/// <summary>
		/// Gets or sets the certificado.
		/// </summary>
		/// <value>The certificado.</value>
		public X509Certificate Certificado { get; set; }

		/// <summary>
		/// Gets or sets the encoder.
		/// </summary>
		/// <value>The encoder.</value>
		public Encoding Encoder { get; set; }

		/// <summary>
		/// Gets the lista de alertas.
		/// </summary>
		/// <value>The lista de alertas.</value>
		public BindingList<string> ErrosAlertas { get; }

		/// <summary>
		/// Gets or sets the formato alerta.
		/// </summary>
		/// <value>The formato alerta.</value>
		public string FormatoAlerta { get; set; }

		#endregion Propriedades

		#region Methods

		/// <summary>
		/// Ws the alerta.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="descricao">The descricao.</param>
		/// <param name="alerta">The alerta.</param>
		internal void WAlerta(string id, string tag, string descricao, string alerta)
		{
			// O Formato da mensagem de erro pode ser alterado pelo usuario alterando-se a property FormatoAlerta: onde;
			// %TAG%       : Representa a TAG; ex: <nLacre>
			// %ID%        : Representa a ID da TAG; ex X34
			// %MSG%       : Representa a mensagem de alerta
			// %DESCRICAO% : Representa a Descri��o da TAG

			var s = FormatoAlerta.Clone() as string;
			if (s == null)
				return;

			s = s.Replace("%ID%", id).Replace("%TAG%", $"<{tag}>").Replace("%DESCRICAO%", descricao).Replace("%MSG%", alerta);

			ErrosAlertas.Add(s);
		}

		#endregion Methods
	}
}