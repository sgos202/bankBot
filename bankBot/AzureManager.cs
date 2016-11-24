using bankBot.dataModel;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace bankBot
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Branch> branchTable;
        private IMobileServiceTable<Accounts> accountsTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://phase2sgos202.azurewebsites.net");
            this.branchTable = this.client.GetTable<Branch>();
            this.accountsTable = this.client.GetTable<Accounts>();
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
        
        

        public async Task<List<Branch>> GetBranches()
        {
            return await this.branchTable.ToListAsync();
        }
        public async Task<List<Accounts>> GetAccounts()
        {
            return await this.accountsTable.ToListAsync();
        }

        public async Task AddAccounts(Accounts accounts)
        {
            await this.accountsTable.InsertAsync(accounts);
        }
        public async Task AddBranches(Branch branches)
        {
            await this.branchTable.InsertAsync(branches);
        }

        public async Task DeleteBranches(Branch branches)
        {
            await this.branchTable.DeleteAsync(branches);
        }

    }
}