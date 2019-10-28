using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StEn.Browservus.BrowserApi.Browser.EvalEntities
{
	[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
	public class EvalResponse<T>
	{
		[JsonProperty(Required = Required.Always)]
		[DefaultValue(false)]
		public bool IsSuccess { get; private set; }

		[JsonProperty(Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
		public T Content { get; private set; }

		[JsonProperty(Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
		public string ErrorMessage { get; private set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
