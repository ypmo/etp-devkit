﻿//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2018 Energistics
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

using Avro.IO;
using Energistics.Etp.Common;
using Energistics.Etp.Common.Datatypes;
using Energistics.Etp.v12.Datatypes.Object;

namespace Energistics.Etp.v12.Protocol.Store
{
    /// <summary>
    /// Base implementation of the <see cref="IStoreCustomer"/> interface.
    /// </summary>
    /// <seealso cref="Energistics.Etp.Common.EtpProtocolHandler" />
    /// <seealso cref="Energistics.Etp.v12.Protocol.Store.IStoreCustomer" />
    public class StoreCustomerHandler : EtpProtocolHandler, IStoreCustomer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreCustomerHandler"/> class.
        /// </summary>
        public StoreCustomerHandler() : base((int)Protocols.Store, "customer", "store")
        {
        }

        /// <summary>
        /// Sends a GetObject message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="messageFlag">The message flag.</param>
        /// <returns>The message identifier.</returns>
        public virtual long GetObject(string uri, MessageFlags messageFlag = MessageFlags.FinalPart)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.GetObject, messageFlags: messageFlag);

            var getObject = new GetObject()
            {
                Uri = uri
            };

            return Session.SendMessage(header, getObject);
        }

        /// <summary>
        /// Sends a PutObject message to a store.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>The message identifier.</returns>
        public virtual long PutObject(DataObject dataObject)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.PutObject);

            var putObject = new PutObject()
            {
                DataObject = dataObject
            };

            return Session.SendMessage(header, putObject);
        }

        /// <summary>
        /// Sends a DeleteObject message to a store.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The message identifier.</returns>
        public virtual long DeleteObject(string uri)
        {
            var header = CreateMessageHeader(Protocols.Store, MessageTypes.Store.DeleteObject);

            var deleteObject = new DeleteObject()
            {
                Uri = uri
            };

            return Session.SendMessage(header, deleteObject);
        }

        /// <summary>
        /// Handles the Object event from a store.
        /// </summary>
        public event ProtocolEventHandler<Object> OnObject;

        /// <summary>
        /// Decodes the message based on the message type contained in the specified <see cref="IMessageHeader" />.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="decoder">The message decoder.</param>
        /// <param name="body">The message body.</param>
        protected override void HandleMessage(IMessageHeader header, Decoder decoder, string body)
        {
            switch (header.MessageType)
            {
                case (int)MessageTypes.Store.Object:
                    HandleObject(header, decoder.Decode<Object>(body));
                    break;

                default:
                    base.HandleMessage(header, decoder, body);
                    break;
            }
        }

        /// <summary>
        /// Handles the Object message from a store.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="object">The Object message.</param>
        protected virtual void HandleObject(IMessageHeader header, Object @object)
        {
            Notify(OnObject, header, @object);
        }
    }
}
