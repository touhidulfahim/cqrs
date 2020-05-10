using CitiBank.View.Views.Accounts.Dtos;
using Xendor.QueryModel.Attributes;
using Xendor.QueryModel.Criteria;

namespace CitiBank.View.Views.Accounts.Criterias
{
    [EmbedField("operations" , typeof(OperationDto))]
    public class AccountCriteria : IMetaDataExpression
    {
        [Field("number")]
        public string Number { get; set; }
       
        [DeepField("product")]
        public ProductCriteria Product { get; set; }

        [DeepField("client")]
        public ClientCriteria Client { get; set; }

    }
}