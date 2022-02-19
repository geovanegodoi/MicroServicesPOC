using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using NSubstitute;

namespace MSPOC.Catalog.Service.UnitTest.TestDoubles
{
    public class ConsumeContextStub<T>
        where T : class
    {
        private readonly T _message;
        private readonly ConsumeContext<T> _stub;
        public ConsumeContext<T> GetContext() => _stub;
        public T GetMessage() => _stub.Message;

        public ConsumeContextStub(T message)
        {
            _message = message;
            _stub = Substitute.For<ConsumeContext<T>>();
            _stub.Message.Returns(_message);
        }
    }
}