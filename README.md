# Firewall Password Encryption/Decryption

This console application allows users to encrypt and decrypt passwords using AES-GCM encryption with a passphrase. The application supports commands for encrypting and decrypting passwords securely.

## Features

- Encrypt passwords with a passphrase.
- Decrypt encrypted passwords using the same passphrase.
- Simple command-line interface.
- Supports help command to list available commands.

## Commands

- **Encrypt a Password**
  ```
  enc -f <passphrase> -p <your_password>
  ```
  This command encrypts `<your_password>` using the specified `<passphrase>`.

- **Decrypt a Password**
  ```
  dec -f <passphrase> -p <your_password>
  ```
  This command decrypts the `<your_password>` (which is expected to be an encrypted string) using the specified `<passphrase>`.

- **Exit the Console**
  ```
  exit
  ```
  This command exits the application.

- **Help Command**
  ```
  --help
  ```
  This command displays a list of available commands and their descriptions.

## Usage

1. Clone this repository:
   ```bash
   git clone https://github.com/yourusername/your-repo.git
   cd your-repo
   ```

2. Open the project in your favorite IDE or text editor.

3. Build the project using .NET CLI:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

5. Use the commands as described above in the console.

## Example

To encrypt a password:
```
f_enc_dec> enc -f mySecretPassphrase -p myPassword123
```

To decrypt an encrypted password:
```
f_enc_dec> dec -f mySecretPassphrase -p <encrypted_password>
```

## Error Handling

The application includes error handling for the following scenarios:
- Invalid passphrase or corrupted encrypted data during decryption.
- Invalid command format.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Feel free to submit issues or pull requests. Contributions are welcome!
