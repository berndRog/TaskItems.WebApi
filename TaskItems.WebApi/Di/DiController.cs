namespace TaskItems.Di;

public static class DiController {
   public static IServiceCollection AddDiController(
      this IServiceCollection serviceCollection
   ) {
      serviceCollection.AddControllers();     
      return serviceCollection;
   }
}