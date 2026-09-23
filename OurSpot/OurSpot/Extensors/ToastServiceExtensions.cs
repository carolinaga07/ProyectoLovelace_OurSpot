using BlazorBootstrap;

namespace OurSpot.Extensors
{
    public static class ToastServiceExtensions
    {
        public static ToastMessage ShowToast(this ToastService toastService, ToastType toastType, string title, string customMessage = null)
        {
            var message = new ToastMessage()
            {
                Type = toastType,
                Title = title,
                Message = customMessage ?? $"A las {DateTime.Now.ToString("hh:mm tt")}"
            };
            toastService.Notify(message);
            return message;
        }

        public static ToastMessage ShowSuccess(this ToastService toastService, string customMessage = null, string title = "Success")
        {
            return toastService.ShowToast(ToastType.Success, title, customMessage);
        }

        public static ToastMessage ShowWarning(this ToastService toastService, string customMessage = null, string title = "Warning")
        {
            return toastService.ShowToast(ToastType.Warning, title, customMessage);
        }

        public static ToastMessage ShowError(this ToastService toastService, string customMessage = null, string title = "Error")
        {
            return toastService.ShowToast(ToastType.Danger, title, customMessage);
        }

    }
}
