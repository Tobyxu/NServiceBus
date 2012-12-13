﻿namespace NServiceBus.RabbitMQ
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;

    public static class RabbitMqTransportMessageExtensions
    {
        public static IBasicProperties FillRabbitMqProperties(this TransportMessage message, IBasicProperties properties)
        {
            properties.MessageId = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(message.CorrelationId))
                properties.CorrelationId = message.CorrelationId;

            if (message.TimeToBeReceived < TimeSpan.MaxValue)
                properties.Expiration = message.TimeToBeReceived.TotalMilliseconds.ToString();

            properties.SetPersistent(message.Recoverable);

            if (message.Headers != null)
            {
                properties.Headers = message.Headers;

                if (message.Headers.ContainsKey(Headers.EnclosedMessageTypes))
                {
                    properties.Type = message.Headers[Headers.EnclosedMessageTypes];
                }

                if (message.Headers.ContainsKey(Headers.ContentType))
                    properties.ContentType = message.Headers[Headers.ContentType];

                if (message.ReplyToAddress != null && message.ReplyToAddress != Address.Undefined)
                    properties.ReplyTo = message.ReplyToAddress.Queue;

            }
            else
            {
                properties.Headers = new Dictionary<string, string>();
            }

            properties.AppId = message.MessageIntent.ToString();

            return properties;
        }



        public static TransportMessage ToTransportMessage(this BasicDeliverEventArgs message)
        {

            var properties = message.BasicProperties;
            var result = new TransportMessage
                {
                    Body = message.Body
                };

            if (properties.IsReplyToPresent())
                result.ReplyToAddress = Address.Parse(properties.ReplyTo);


            result.Headers = message.BasicProperties.Headers.Cast<DictionaryEntry>()
                                        .ToDictionary(
                                        kvp => (string)kvp.Key, 
                                        kvp => Encoding.UTF8.GetString((byte[])kvp.Value));
            if (properties.IsAppIdPresent())
                result.MessageIntent = (MessageIntentEnum)Enum.Parse(typeof(MessageIntentEnum), properties.AppId);

            if (properties.IsCorrelationIdPresent())
                result.CorrelationId = properties.CorrelationId;

            return result;
        }
    }

}