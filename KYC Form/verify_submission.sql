SELECT 
    k.Id,
    k.SubmissionDate,
    k.FirstName + ' ' + ISNULL(k.MiddleName + ' ', '') + k.LastName AS FullName,
    k.Email,
    k.Mobile,
    k.AadhaarNumber,
    at.TypeName AS AccountType,
    k.Status
FROM KYCSubmissions k
LEFT JOIN AccountTypes at ON k.AccountTypeId = at.Id
ORDER BY k.SubmissionDate DESC;
