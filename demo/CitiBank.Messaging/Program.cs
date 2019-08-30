﻿using System.IO;
using CitiBank.Messaging.Filters;
using CitiBank.Messaging.Filters.Consumers;
using Microsoft.Extensions.Configuration;
using Xendor.Data.MySql;
using Xendor.EventBus.RabbitMQ;
using Xendor.MessageModel.MessageBroker;

namespace CitiBank.Messaging
{
    class Program
    {
        static void  Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            var connectionStringSection = configuration.GetSection("connectionString");
            var unitOfWorkConnection = connectionStringSection.Get<MySqlConnection>();



            var unitOfWorkFactory = new MySqlUnitOfWorkFactory(unitOfWorkConnection);
            var messageBroker = new QueryMessageBroker(unitOfWorkFactory);
            messageBroker.Bind<ProductCreateQueryMessageFilter>();
            messageBroker.Bind<ClientCreateQueryMessageFilter>();
            messageBroker.Bind<AccountCreateQueryMessageFilter>();
            messageBroker.Bind<AccountUpdateQueryMessageFilter>();

            var rabbitConnectionStringSection = configuration.GetSection("rabbitConnectionString");
            var rabbitMqConnectionString = rabbitConnectionStringSection.Get<RabbitMqConnectionString>();

            var eventBus = new RabbitMqEventBus(rabbitMqConnectionString);
            eventBus.Subscribe("ProductCreated", ProductCreatedEventConsumer.ConsumerFactory(messageBroker));
            eventBus.Subscribe("ClientCreated", ClientCreatedEventConsumer.ConsumerFactory(messageBroker));
            eventBus.Subscribe("AccountCreated", AccountCreatedEventConsumer.ConsumerFactory(messageBroker));
            eventBus.Subscribe("AccountBalanceChanged", AccountBalanceChangedEventConsumer.ConsumerFactory(messageBroker));
            eventBus.Subscribe("AccountTransfered", AccountTransferedEventConsumer.ConsumerFactory(messageBroker));
        }
    }
}