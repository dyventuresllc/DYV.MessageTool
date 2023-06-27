INSERT INTO EDDS.mt.EmailQueue(ArtifactID, MsgArtifactID, FirstName, EmailAddress, [Subject], [Body])
SELECT
	eu.ArtifactID, @MsgArtifactID, eu.FirstName, eu.EmailAddress, @Subject, @Body
FROM EDDS.eddsdbo.[ExtendedUser] eu WITH (NOLOCK)
JOIN EDDS.eddsdbo.Artifact a WITH (NOLOCK)
	ON eu.EmailPreference = a.ArtifactID
LEFT JOIN EDDS.mt.EmailQueue eq
	ON eq.ArtifactID = eu.ArtifactID
WHERE 
	SUBSTRING(eu.EmailAddress,CHARINDEX('@',eu.EmailAddress)+1,(LEN(eu.EmailAddress)-CHARINDEX('@',eu.EmailAddress))) NOT IN ('relativity.com','archived','kcura.com','PreviewUser.com')
AND
	a.TextIdentifier NOT LIKE 'No Emails' --EmailPreference
AND 
	eu.RelativityAccess = 1
AND
	eq.ArtifactID IS NULL;