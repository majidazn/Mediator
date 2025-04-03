namespace Mediator
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No handler registered for {request.GetType().Name}");

            var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))
                ?? throw new InvalidOperationException($"Handler for {request.GetType().Name} does not implement Handle method");

            var del = (Func<object, IRequest<TResponse>, CancellationToken, Task<TResponse>>)Delegate.CreateDelegate(
                typeof(Func<object, IRequest<TResponse>, CancellationToken, Task<TResponse>>), method);

            return await del(handler, request, cancellationToken);
        }

        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            var handlerType = typeof(INotificationHandler<>).MakeGenericType(typeof(TNotification));
            var handlers = _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType)) as IEnumerable<object>;

            if (handlers == null || !handlers.Any()) return;

            var tasks = handlers.Select(handler =>
            {
                var method = handlerType.GetMethod(nameof(INotificationHandler<TNotification>.Handle))
                    ?? throw new InvalidOperationException($"Handler {handler.GetType().Name} does not implement Handle method");

                var del = (Func<object, TNotification, CancellationToken, Task>)Delegate.CreateDelegate(
                    typeof(Func<object, TNotification, CancellationToken, Task>), method);

                return del(handler, notification, cancellationToken);
            });

            await Task.WhenAll(tasks);
        }
    }
}
