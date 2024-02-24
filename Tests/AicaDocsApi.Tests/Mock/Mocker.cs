using AicaDocsApi.Database;
using AicaDocsApi.Utils.BlobServices;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;

namespace AicaDocsApi.Tests.Mock;

public static class Mocker
{
    public static AicaDocsDb BuildMockAicaDocsDb()
    {
        // var dbContextMock = new Mock<AicaDocsDb>();
        // dbContextMock.Setup(x => x.Documents).ReturnsDbSet(Faker.GetFakeDocumentList());
        // dbContextMock.Setup(x => x.Downloads).ReturnsDbSet(Faker.GetFakeDownloadList());
        // dbContextMock.Setup(x => x.Nomenclators).ReturnsDbSet(Faker.GetFakeNomenclatorList());

        // var dbContextMock = new Mock<AicaDocsDb>();
        // dbContextMock.Setup(x => x.Documents)
        //     .ReturnsDbSet(Faker.GetFakeDocumentList().AsQueryable().BuildMockDbSet().Object);
        // dbContextMock.Setup(x => x.Downloads)
        //     .ReturnsDbSet(Faker.GetFakeDownloadList().AsQueryable().BuildMockDbSet().Object);
        // dbContextMock.Setup(x => x.Nomenclators)
        //     .ReturnsDbSet(Faker.GetFakeNomenclatorList().AsQueryable().BuildMockDbSet().Object);
        
        // var documents = Faker.GetFakeDocumentList().AsQueryable();
        // var downloads = Faker.GetFakeDownloadList().AsQueryable();
        // var nomenclators = Faker.GetFakeNomenclatorList().AsQueryable();
        //
        // var dbContextMock = new Mock<AicaDocsDb>();
        // dbContextMock.Setup(x => x.Documents).ReturnsDbSet(documents.BuildMock());
        // dbContextMock.Setup(x => x.Downloads).ReturnsDbSet(downloads.BuildMock());
        // dbContextMock.Setup(x => x.Nomenclators).ReturnsDbSet(nomenclators.BuildMock());
        
        var documents = Faker.GetFakeDocumentList().AsQueryable().BuildMockDbSet().Object;
        var downloads = Faker.GetFakeDownloadList().AsQueryable().BuildMockDbSet().Object;
        var nomenclators = Faker.GetFakeNomenclatorList().AsQueryable().BuildMockDbSet().Object;
        
        var dbContextMock = new Mock<AicaDocsDb>();
        dbContextMock.Setup(x => x.Documents).Returns(documents);
        dbContextMock.Setup(x => x.Downloads).Returns(downloads);
        dbContextMock.Setup(x => x.Nomenclators).Returns(nomenclators);
        
        // dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
        //     .ReturnsAsync(1);

        return dbContextMock.Object;
    }

    public static IBlobService BuildMockIBlobService()
    {
        var blobServiceMock = new Mock<IBlobService>();

        blobServiceMock.Setup(x =>
                x.UploadObject(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        blobServiceMock.Setup(x => x.PresignedGetUrl(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult("http://example.com/presigned-url"));

        blobServiceMock.Setup(x => x.ValidateExistanceObject(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));

        return blobServiceMock.Object;
    }
}