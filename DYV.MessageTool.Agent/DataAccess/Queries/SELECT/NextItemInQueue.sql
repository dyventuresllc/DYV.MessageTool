BEGIN TRAN [UpdateAgentQueue]

	DECLARE @QueueID INT;

	SELECT TOP (1)
		@QueueID = mt.ArtifactID
	FROM EDDS.mt.EmailQueue mt
	WHERE mt.AgentID = 0

	UPDATE mt 
		SET	mt.[AgentID] = @AgentID
	FROM EDDS.mt.EmailQueue mt
	WHERE mt.ArtifactID = @QueueID

	SELECT
		ArtifactID
		,WorkspaceID
		,MsgArtifactID
		,FirstName
		,EmailAddress
		,[Subject]
		,Body
		,AgentID
	FROM EDDS.mt.EmailQueue
	WHERE ArtifactID = @QueueID

COMMIT TRAN [UpdateAgentQueue]	