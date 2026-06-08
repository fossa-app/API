using System.Net;
using System.Net.Http;
using Fossa.Bridge.Models.ApiModels;
using Microsoft.FSharp.Core;
using Shouldly;

namespace Fossa.API.FunctionalTests;

internal static class ClientResultExtensions
{
  public static T Unwrap<T>(this ClientResult<T> result)
    where T : class
    => result.Match(
      FuncConvert.FromFunc<T, T>(value => value),
      FuncConvert.FromFunc<ProblemDetailsModel, T>(problemDetails =>
        throw new HttpRequestException(
          $"Client request failed with status code {problemDetails.Status}. {result}",
          inner: null,
          statusCode: (System.Net.HttpStatusCode)problemDetails.Status)));

  public static ProblemDetailsModel ShouldFailWith<T>(this ClientResult<T> result, HttpStatusCode statusCode)
    where T : class
    => result.Match(
      FuncConvert.FromFunc<T, ProblemDetailsModel>(_ =>
      {
        result.IsFailure.ShouldBeTrue();
        return null!;
      }),
      FuncConvert.FromFunc<ProblemDetailsModel, ProblemDetailsModel>(problemDetails =>
      {
        problemDetails.Status.ShouldBe((int)statusCode);
        return problemDetails;
      }));

  public static ProblemDetailsModel ShouldFailWith(this ClientUnitResult result, HttpStatusCode statusCode)
    => result.Match(
      FuncConvert.FromFunc<Microsoft.FSharp.Core.Unit, ProblemDetailsModel>(_ =>
      {
        result.IsFailure.ShouldBeTrue();
        return null!;
      }),
      FuncConvert.FromFunc<ProblemDetailsModel, ProblemDetailsModel>(problemDetails =>
      {
        problemDetails.Status.ShouldBe((int)statusCode);
        return problemDetails;
      }));
}
