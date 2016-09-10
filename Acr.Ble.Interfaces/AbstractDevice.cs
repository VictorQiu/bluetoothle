﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;


namespace Acr.Ble
{
    public abstract class AbstractDevice : IDevice
    {
        protected IDictionary<Guid, IGattService> Services { get; }


        protected AbstractDevice(string initialName, Guid uuid)
        {
            this.Name = initialName;
            this.Uuid = uuid;
            this.Services = new Dictionary<Guid, IGattService>();
        }


        public string Name { get; protected set; }
        public Guid Uuid { get; protected set; }
        public abstract ConnectionStatus Status { get; }

        public abstract void Disconnect();
        public abstract IObservable<object> Connect();
        public abstract IObservable<int> WhenRssiUpdated(TimeSpan? timeSpan);
        public abstract IObservable<ConnectionStatus> WhenStatusChanged();
        public abstract IObservable<IGattService> WhenServiceDiscovered();
        public abstract IObservable<string> WhenNameUpdated();


        public virtual IObservable<ConnectionStatus> PersistentConnect()
        {
            return Observable.Create<ConnectionStatus>(async ob =>
            {
                var state = this.WhenStatusChanged().Subscribe(ob.OnNext);
                await this.Connect();

                return () =>
                {
                    state.Dispose();
                    this.Disconnect();
                };
            });
        }
    }
}