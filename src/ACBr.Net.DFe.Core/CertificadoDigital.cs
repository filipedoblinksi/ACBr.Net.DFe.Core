// ***********************************************************************
// Assembly         : ACBr.Net.Core
// Author           : arezende
// Created          : 07-27-2014
//
// Last Modified By : RFTD
// Last Modified On : 09-02-2014
// ***********************************************************************
// <copyright file="CertificadoDigital.cs" company="ACBr.Net">
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

using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Schema;

namespace ACBr.Net.DFe.Core
{
	/// <summary>
	/// Classe CertificadoDigital.
	/// </summary>
	public static class CertificadoDigital
	{
		#region Methods

		/// <summary>
		/// Assina a XML usando o certificado informado.
		/// </summary>
		/// <param name="xml">A XML.</param>
		/// <param name="pUri">A Url.</param>
		/// <param name="pNode">The p node.</param>
		/// <param name="pCertificado">O certificado.</param>
		/// <param name="comments">if set to <c>true</c> [comments].</param>
		/// <returns>System.String.</returns>
		/// <exception cref="Exception">Erro ao efetuar assinatura digital, detalhes:  + ex.Message</exception>
		/// <exception cref="System.Exception">Erro ao efetuar assinatura digital, detalhes:  + ex.Message</exception>
		public static string Assinar(string xml, string pUri, string pNode, X509Certificate2 pCertificado, bool comments = false)
		{
			try
			{
				var doc = new XmlDocument();
				doc.LoadXml(xml);

				//Adiciona Certificado ao Key Info
				var keyInfo = new KeyInfo();
				keyInfo.AddClause(new KeyInfoX509Data(pCertificado));

				//Seta chaves
				var signedDocument = new SignedXml(doc)
				{
					SigningKey = pCertificado.PrivateKey,
					KeyInfo = keyInfo
				};

				// Cria referencia
				var reference = new Reference { Uri = pUri };

				// Adiciona transforma��o a referencia
				reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
				reference.AddTransform(new XmlDsigC14NTransform(comments));

				// Adiciona referencia ao xml
				signedDocument.AddReference(reference);

				// Calcula Assinatura
				signedDocument.ComputeSignature();

				// Pega representa��o da assinatura
				var xmlDigitalSignature = signedDocument.GetXml();

				// Adiciona ao doc XML
				var xmlElement = doc.GetElementsByTagName(pNode)[0] as XmlElement;
				Guard.Against<ArgumentException>(xmlElement == null, "Nome do elemento de assinatura incorreto");
				xmlElement.ParentNode.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

				return doc.AsString();
			}
			catch (Exception ex)
			{
				throw new Exception("Erro ao efetuar assinatura digital, detalhes: " + ex.Message);
			}
		}

		/// <summary>
		/// Busca certificados instalado se informado uma serie
		/// se n�o abre caixa de dialogos de certificados.
		/// </summary>
		/// <param name="cerSerie">Serie do certificado.</param>
		/// <returns>X509Certificate2.</returns>
		/// <exception cref="System.Exception">
		/// Nenhum certificado digital foi selecionado ou o certificado selecionado est� com problemas.
		/// or
		/// Certificado digital n�o encontrado
		/// or
		/// </exception>
		public static X509Certificate2 SelecionarCertificado(string cerSerie)
		{
			var certificate = new X509Certificate2();
			try
			{
				var store = new X509Store("MY", StoreLocation.CurrentUser);
				store.Open(OpenFlags.OpenExistingOnly);
				var certificates = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true)
					.Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, true);

				X509Certificate2Collection certificatesSel;
				if (cerSerie.IsEmpty())
				{
					certificatesSel = X509Certificate2UI.SelectFromCollection(certificates, "Certificados Digitais", "Selecione o Certificado Digital para uso no aplicativo", X509SelectionFlag.SingleSelection);
					if (certificatesSel.Count == 0)
					{
						certificate.Reset();
						throw new ACBrDFeException("Nenhum certificado digital foi selecionado ou o certificado selecionado est� com problemas.");
					}

					certificate = certificatesSel[0];
				}
				else
				{
					certificatesSel = certificates.Find(X509FindType.FindBySerialNumber, cerSerie, true);
					if (certificatesSel.Count == 0)
					{
						certificate.Reset();
						throw new ACBrDFeException("Certificado digital n�o encontrado");
					}

					certificate = certificatesSel[0];
				}

				store.Close();
				return certificate;
			}
			catch (Exception ex)
			{
				throw new ACBrDFeException("Erro ao selecionar o certificado", ex);
			}
		}

		/// <summary>
		/// Seleciona um certificado informando o caminho e a senha.
		/// </summary>
		/// <param name="caminho">O caminho.</param>
		/// <param name="senha">A senha.</param>
		/// <returns>X509Certificate2.</returns>
		/// <exception cref="System.Exception">Arquivo do Certificado digital n�o encontrado</exception>
		public static X509Certificate2 SelecionarCertificado(string caminho, string senha)
		{
			Guard.Against<ArgumentNullException>(caminho.IsEmpty(), "Caminho do arquivo n�o poder ser nulo ou vazio !");
			Guard.Against<ArgumentException>(!File.Exists(caminho), "Arquivo do Certificado digital n�o encontrado !");

			var cert = new X509Certificate2(caminho, senha, X509KeyStorageFlags.MachineKeySet);
			return cert;
		}

		/// <summary>
		/// Validars the XML.
		/// </summary>
		/// <param name="arquivoXml">The arquivo XML.</param>
		/// <param name="schema">The schema nf.</param>
		/// <param name="erros">The erro.</param>
		/// <param name="avisos">The avisos.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool ValidarXml(string arquivoXml, string schema, out string[] erros, out string[] avisos)
		{
			var errorList = new List<string>();
			var avisosList = new List<string>();

			if (string.IsNullOrEmpty(arquivoXml))
			{
				errorList.Add("Arquivo da nota fiscal n�o encontrado.");
				erros = errorList.ToArray();
				avisos = avisosList.ToArray();
				return false;
			}

			if (!File.Exists(schema))
			{
				errorList.Add("Arquivo de Schema n�o encontrado.");
				erros = errorList.ToArray();
				avisos = avisosList.ToArray();
				return false;
			}

			try
			{
				var settings = new XmlReaderSettings();
				var schema2 = XmlSchema.Read(new XmlTextReader(schema), (sender, args) =>
				{
					switch (args.Severity)
					{
						case XmlSeverityType.Warning:
							// ReSharper disable once AccessToModifiedClosure
							avisosList.Add(args.Message);
							break;

						case XmlSeverityType.Error:
							// ReSharper disable once AccessToModifiedClosure
							errorList.Add(args.Message);
							break;
					}

					// Erro na valida��o do schema XSD
					if ((args.Exception != null))
					{
						// ReSharper disable once AccessToModifiedClosure
						errorList.Add("\nErro: " + args.Exception.Message + "\nLinha " + args.Exception.LinePosition + " - Coluna "
								  + args.Exception.LineNumber + "\nSource: " + args.Exception.SourceUri);
					}
				});

				settings.ValidationType = ValidationType.Schema;
				settings.Schemas.Add(schema2);

				var input = new StringReader(arquivoXml);
				using (var reader2 = XmlReader.Create(input, settings))
				{
					while (reader2.Read())
					{
					}
				}
			}
			catch (Exception exception)
			{
				errorList.Add(exception.Message);
			}

			erros = errorList.ToArray();
			avisos = avisosList.ToArray();
			errorList = null;
			avisosList = null;

			return (erros.Length < 1);
		}

		#endregion Methods
	}
}