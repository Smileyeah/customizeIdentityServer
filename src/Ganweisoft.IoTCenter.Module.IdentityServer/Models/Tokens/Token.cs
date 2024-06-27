using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.Models.Tokens;


/// <summary>
/// Models a token.
/// </summary>
public class Token
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Token"/> class.
    /// </summary>
    public Token()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="tokenType">Type of the token.</param>
    public Token(string tokenType)
    {
        Type = tokenType;
    }

    /// <summary>
    /// A list of allowed algorithm for signing the token. If null or empty, will use the default algorithm.
    /// </summary>
    public ICollection<string> AllowedSigningAlgorithms { get; set; } = new HashSet<string>();

    /// <summary>
    /// Specifies the confirmation method of the token. This value, if set, will become the cnf claim.
    /// </summary>
    public string Confirmation { get; set; }

    /// <summary>
    /// Gets or sets the audiences.
    /// </summary>
    /// <value>
    /// The audiences.
    /// </value>
    public ICollection<string> Audiences { get; set; } = new HashSet<string>();
    
    /// <summary>
    /// Gets or sets the issuer.
    /// </summary>
    /// <value>
    /// The issuer.
    /// </value>
    public string Issuer { get; set; }
    
    /// <summary>
    /// Gets or sets the creation time.
    /// </summary>
    /// <value>
    /// The creation time.
    /// </value>
    public DateTime CreationTime { get; set; }
    
    /// <summary>
    /// Gets or sets the lifetime.
    /// </summary>
    /// <value>
    /// The lifetime.
    /// </value>
    public int Lifetime { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public string Type { get; set; } = "access_token";

    /// <summary>
    /// Gets or sets the ID of the client.
    /// </summary>
    /// <value>
    /// The ID of the client.
    /// </value>
    public string AppId { get; set; }

    /// <summary>
    /// Gets the description the user assigned to the device being authorized.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the claims.
    /// </summary>
    /// <value>
    /// The claims.
    /// </value>
    public ICollection<Claim> Claims { get; set; } = new HashSet<Claim>();
    
    /// <summary>
    /// Gets the scopes.
    /// </summary>
    public IEnumerable<string> Scopes => this.Claims.Where(x => x.Type == IdentityServerConstant.GwClaimTypesScope).Select((Func<Claim, string>) (x => x.Value));
}