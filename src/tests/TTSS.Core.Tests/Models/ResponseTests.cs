using AutoFixture;
using FluentAssertions;
using System.Net;
using TTSS.Tests;

namespace TTSS.Core.Models;

public class ResponseTests : TestBase
{
    [Fact]
    public void CreateResponse_ShouldBeCreatedCorrectly()
    {
        var responseMessage = Fixture.Create<string>();
        var sut = new Response(responseMessage);
        sut.Should().BeEquivalentTo(new { Message = responseMessage });
    }

    [Fact]
    public void CreateResponse_WithPrimitiveDataType_ShouldBeCreatedCorrectly()
    {
        var responseData = Fixture.Create<double>();
        var responseMessage = Fixture.Create<string>();
        var sut = new Response<double>(responseMessage, responseData);
        sut.Should().BeEquivalentTo(new { Message = responseMessage, Data = responseData });
    }

    [Fact]
    public void CreateResponse_WithSimpleDataType_ShouldBeCreatedCorrectly()
    {
        var responseData = Fixture.Create<SimpleType>();
        var responseMessage = Fixture.Create<string>();
        var sut = new Response<SimpleType>(responseMessage, responseData);
        sut.Should().BeEquivalentTo(new { Message = responseMessage, Data = responseData });
    }

    [Fact]
    public void CreateResponse_WithComplexDataType_ShouldBeCreatedCorrectly()
    {
        var responseData = Fixture.Create<ComplexType<SimpleType>>();
        var responseMessage = Fixture.Create<string>();
        var sut = new Response<ComplexType<SimpleType>>(responseMessage, responseData);
        sut.Should().BeEquivalentTo(new { Message = responseMessage, Data = responseData });
    }

    [Fact]
    public void CreateHttpResponse_WithPrimitiveDataType_ShouldBeCreatedCorrectly()
    {
        var responseData = Fixture.Create<double>();
        var responseMessage = Fixture.Create<string>();
        var sut = new HttpResponse<double>(HttpStatusCode.OK, responseMessage, responseData);
        sut.Should().BeEquivalentTo(new { StatusCode = HttpStatusCode.OK, Message = responseMessage, Data = responseData });
    }

    [Fact]
    public void CreateHttpResponse_WithSimpleDataType_ShouldBeCreatedCorrectly()
    {
        var responseData = Fixture.Create<SimpleType>();
        var responseMessage = Fixture.Create<string>();
        var sut = new HttpResponse<SimpleType>(HttpStatusCode.OK, responseMessage, responseData);
        sut.Should().BeEquivalentTo(new { StatusCode = HttpStatusCode.OK, Message = responseMessage, Data = responseData });
    }

    [Fact]
    public void CreateHttpResponse_WithComplexDataType_ShouldBeCreatedCorrectly()
    {
        var responseData = Fixture.Create<ComplexType<SimpleType>>();
        var responseMessage = Fixture.Create<string>();
        var sut = new HttpResponse<ComplexType<SimpleType>>(HttpStatusCode.OK, responseMessage, responseData);
        sut.Should().BeEquivalentTo(new { StatusCode = HttpStatusCode.OK, Message = responseMessage, Data = responseData });
    }

    [Fact]
    public void ConvertHttpStatusCode_FromHttpStatusExtension_WithoutData_ShouldBeWorkCorrectly()
    {
        var responseMessage = Fixture.Create<string>();
        var expected = new HttpResponse(HttpStatusCode.OK, responseMessage);

        HttpStatusCode.OK.ToResponse(responseMessage).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ConvertHttpStatusCode_FromHttpStatusExtension_WithData_ShouldBeWorkCorrectly()
    {
        var responseData = Fixture.Create<SimpleType>();
        var responseMessage = Fixture.Create<string>();
        var expected = new HttpResponse<SimpleType>(HttpStatusCode.OK, responseMessage, responseData);

        HttpStatusCode.OK.ToResponse(responseMessage, responseData).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void HttpResponse_AreCompatibleWithResponseBaseTypes()
    {
        var expected = Fixture.Create<HttpResponse>();
        (expected as Response).Should().NotBeNull().And.BeEquivalentTo(expected);
        (expected as IResponse).Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public void HttpResponse_WithData_AreCompatibleWithResponseBaseTypes()
    {
        var expected = Fixture.Create<HttpResponse<SimpleType>>();
        (expected as Response).Should().NotBeNull().And.BeEquivalentTo(expected);
        (expected as IResponse).Should().NotBeNull().And.BeEquivalentTo(expected);
        (expected as Response<SimpleType>).Should().NotBeNull().And.BeEquivalentTo(expected);
        (expected as IResponse<SimpleType>).Should().NotBeNull().And.BeEquivalentTo(expected);
    }
}

file class SimpleType
{
    public int Age { get; set; }
    public string Name { get; set; }
}

file class ComplexType<TInner>
{
    public IEnumerable<TInner> Inners { get; set; }
}