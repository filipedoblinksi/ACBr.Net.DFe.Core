using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Attributes;
using ACBr.Net.DFe.Core.Extensions;
using ACBr.Net.DFe.Core.Interfaces;
using ACBr.Net.DFe.Core.Internal;

namespace ACBr.Net.DFe.Core.Serializer
{
	internal static class PrimitiveSerializer
	{
		#region Serialize

		/// <summary>
		/// Serializes a fundamental primitive object (e.g. string, int etc.) into a XElement using options.
		/// </summary>
		/// <param name="tag">The name of the primitive to serialize.</param>
		/// <param name="item">The item.</param>
		/// <param name="prop">The property.</param>
		/// <param name="options">Indicates how the output is formatted or serialized.</param>
		/// <returns>The XElement representation of the primitive.</returns>
		public static XObject Serialize(IDFeElement tag, object item, PropertyInfo prop, SerializerOptions options)
		{
			try
			{
				var value = prop.GetValue(item, null);
				var estaVazio = value == null;
				var conteudoProcessado = ProcessValue(ref estaVazio, tag.Tipo, value, tag.Min, prop, item);

				string alerta;
				if (tag.Ocorrencias == 1 && estaVazio && tag.Min > 0)
					alerta = SerializerOptions.ErrMsgVazio;
				else
					alerta = string.Empty;

				if (!string.IsNullOrEmpty(conteudoProcessado.Trim()) &&
					(conteudoProcessado.Length < tag.Min && string.IsNullOrEmpty(alerta) && conteudoProcessado.Length > 1))
					alerta = SerializerOptions.ErrMsgMenor;

				if (!string.IsNullOrEmpty(conteudoProcessado.Trim()) && conteudoProcessado.Length > tag.Max)
					alerta = SerializerOptions.ErrMsgMaior;

				if (!string.IsNullOrEmpty(alerta.Trim()) && SerializerOptions.ErrMsgVazio.Equals(alerta) && !estaVazio)
					alerta += $" [{value}]";

				options.WAlerta(tag.Id, tag.Name, tag.Descricao, alerta);

				XObject xmlTag = null;
				if (tag.Ocorrencias == 1 && estaVazio)
					xmlTag = tag is DFeElementAttribute ? (XObject)new XElement(tag.Name) : new XAttribute(tag.Name, "");

				if (estaVazio)
					return xmlTag;

				var retValue = options.RemoverAcentos ? conteudoProcessado.RemoveCe() : conteudoProcessado;
				xmlTag = tag is DFeElementAttribute ? (XObject)new XElement(tag.Name, retValue) : new XAttribute(tag.Name, retValue);
				return xmlTag;
			}
			catch (Exception ex)
			{
				options.WAlerta(tag.Id, tag.Name, tag.Descricao, ex.ToString());
				return null;
			}
		}

		private static string ProcessValue(ref bool estaVazio, TipoCampo tipo, object valor, int min, PropertyInfo prop, object item)
		{
			var conteudoProcessado = string.Empty;
			// ReSharper disable once SwitchStatementMissingSomeCases
			switch (tipo)
			{
				case TipoCampo.Str:
					if (!estaVazio)
						conteudoProcessado = valor.ToString().Trim();
					break;

				case TipoCampo.Dat:
				case TipoCampo.DatCFe:
					if (!estaVazio)
					{
						DateTime data;
						if (DateTime.TryParse(valor.ToString(), out data))
							conteudoProcessado = data.ToString(tipo == TipoCampo.DatCFe ? "yyyyMMdd" : "yyyy-MM-dd");
						else
							estaVazio = true;
					}
					break;

				case TipoCampo.Hor:
				case TipoCampo.HorCFe:
					if (!estaVazio)
					{
						DateTime hora;
						if (DateTime.TryParse(valor.ToString(), out hora))
							conteudoProcessado = hora.ToString(tipo == TipoCampo.HorCFe ? "HHmmss" : "HH:mm:ss");
						else
							estaVazio = true;
					}
					break;

				case TipoCampo.DatHor:
					if (!estaVazio)
					{
						DateTime dthora;
						if (DateTime.TryParse(valor.ToString(), out dthora))
							conteudoProcessado = dthora.ToString("s");
						else
							estaVazio = true;
					}
					break;

				case TipoCampo.DatHorTz:
					if (!estaVazio)
					{
						DateTime dthoratz;
						if (DateTime.TryParse(valor.ToString(), out dthoratz))
							conteudoProcessado = dthoratz.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
						else
							estaVazio = true;
					}
					break;

				case TipoCampo.De2:
				case TipoCampo.De3:
				case TipoCampo.De4:
				case TipoCampo.De6:
				case TipoCampo.De10:
					if (!estaVazio)
					{
						var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
						decimal vDecimal;
						if (decimal.TryParse(valor.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out vDecimal))
						{
							// ReSharper disable once SwitchStatementMissingSomeCases
							switch (tipo)
							{
								case TipoCampo.De2:
									conteudoProcessado = string.Format(numberFormat, "{0:0.00}", vDecimal);
									break;

								case TipoCampo.De3:
									conteudoProcessado = string.Format(numberFormat, "{0:0.000}", vDecimal);
									break;

								case TipoCampo.De4:
									conteudoProcessado = string.Format(numberFormat, "{0:0.0000}", vDecimal);
									break;

								case TipoCampo.De6:
									conteudoProcessado = string.Format(numberFormat, "{0:0.000000}", vDecimal);
									break;

								default:
									conteudoProcessado = string.Format(numberFormat, "{0:0.0000000000}", vDecimal);
									break;
							}
						}
						else
							estaVazio = true;
					}
					break;

				case TipoCampo.Int:
				case TipoCampo.StrNumberFill:
					if (!estaVazio)
						conteudoProcessado = valor.ToString().ZeroFill(min);
					break;

				case TipoCampo.StrNumber:
					if (!estaVazio)
						conteudoProcessado = valor.ToString().OnlyNumbers();
					break;

				case TipoCampo.Enum:
					if (!estaVazio)
					{
						var member = valor.GetType().GetMember(valor.ToString()).FirstOrDefault();
						var enumAttribute = member?.GetCustomAttributes(false).OfType<DFeEnumAttribute>().FirstOrDefault();
						var enumValue = enumAttribute?.Value;
						conteudoProcessado = enumValue ?? valor.ToString();
					}
					break;

				case TipoCampo.Custom:
					var serialize = item.GetSerializer(prop);
						conteudoProcessado = serialize();
					break;

				default:
					if (!estaVazio)
						conteudoProcessado = valor.ToString();
					break;
			}

			return conteudoProcessado;
		}

		#endregion Deserialize

		#region Deserialize

		/// <summary>
		/// Deserializes the XElement to the fundamental primitive (e.g. string, int etc.) of a specified type using options.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="parentElement">The parent XElement used to deserialize the fundamental primitive.</param>
		/// <param name="item">The item.</param>
		/// <param name="prop">The property.</param>
		/// <returns>The deserialized fundamental primitive from the XElement.</returns>
		public static object Deserialize(IDFeElement tag, XObject parentElement, object item, PropertyInfo prop)
		{
			if (parentElement == null)
				return null;

			var element = parentElement as XElement;
			var value = element?.Value ?? ((XAttribute)parentElement).Value;
			return GetValue(tag.Tipo, value, item, prop);
		}

		private static object GetValue(TipoCampo tipo, string valor, object item, PropertyInfo prop)
		{
			if (valor.IsNull())
				return null;

			object ret;
			// ReSharper disable once SwitchStatementMissingSomeCases
			switch (tipo)
			{
				case TipoCampo.Int:
					ret = valor.ToInt32();
					break;


				case TipoCampo.DatHor:
				case TipoCampo.DatHorTz:
					ret = valor.ToData();
					break;

				case TipoCampo.Dat:
				case TipoCampo.DatCFe:
					ret = DateTime.ParseExact(valor, tipo == TipoCampo.DatCFe ? "yyyyMMdd" : "yyyy-MM-dd",
						CultureInfo.InvariantCulture);
					break;

				case TipoCampo.Hor:
				case TipoCampo.HorCFe:
					ret = DateTime.ParseExact(valor, tipo == TipoCampo.HorCFe ? "HHmmss" : "HH:mm:ss",
						CultureInfo.InvariantCulture);
					break;

				case TipoCampo.De2:
				case TipoCampo.De3:
				case TipoCampo.De4:
				case TipoCampo.De10:
				case TipoCampo.De6:
					var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
					ret = decimal.Parse(valor, numberFormat);
					break;

				case TipoCampo.Enum:
					object value1 = valor;
					foreach (var member in prop.PropertyType.GetMembers().Where(x => x.HasAttribute<DFeEnumAttribute>()))
					{
						var att = member.GetAttribute<DFeEnumAttribute>();
						if (att.Value != valor)
							continue;

						value1 = member.Name;
						break;
					}

					ret = Enum.Parse(prop.PropertyType, value1.ToString());
					break;

				case TipoCampo.Custom:
					var deserialize = item.GetDeserializer(prop);
					ret = deserialize(valor);
					break;

				default:
					ret = valor;
					break;
			}

			return ret;
		}

		#endregion Deserialize
	}
}