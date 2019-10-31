using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


public class Transaction : TableEntity
{
    public Transaction(string contEmitent)
    {
        this.PartitionKey = contEmitent;
        this.RowKey = Guid.NewGuid().ToString();
    }

    public Transaction() { }

    public double suma { get; set; }
    public string numeBeneficiar { get; set; }
    public string numeEmitent { get; set; }
    public string banca { get; set; }
    public string contBeneficiar { get; set; }
    public string contEmitent { get; set; }
}