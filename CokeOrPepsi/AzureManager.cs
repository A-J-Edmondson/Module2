using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CokeOrPepsi.DataModels;

namespace CokeOrPepsi
{
    class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<CokeOrPepsiModel> CokeOrPepsiTable;
        
        private AzureManager()
        { 
            this.client = new MobileServiceClient("http://cokeorpepsi.azurewebsites.net");
            this.CokeOrPepsiTable = this.client.GetTable<CokeOrPepsiModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<CokeOrPepsiModel>> GetCokeOrPepsi()
        {
            return await this.CokeOrPepsiTable.OrderByDescending(c => c.DateNZ).ToListAsync();
        }

        public async Task PostCokeOrPepsi(CokeOrPepsiModel cokeOrPepsiModel)
        {
            await this.CokeOrPepsiTable.InsertAsync(cokeOrPepsiModel);
        }

    }
}
