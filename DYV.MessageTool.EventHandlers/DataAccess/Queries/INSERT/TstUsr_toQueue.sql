INSERT INTO EDDS.mt.EmailQueue(ArtifactID, MsgArtifactID, FirstName, EmailAddress, [Subject], [Body])
VALUES (@UserArtifactID, @MsgArtifactID, @FirstName, @EmailAddress, @Subject, @Body);