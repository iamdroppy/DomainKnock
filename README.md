# DomainKnocker

A bruteforce tool to help find a certain host within a range of IP addresses 

This is a simple project I did for research. I am not responsible for how it is used. 

It will check for port 80/443 for every IP given.

## Docker

Building the image
```bash
git clone https://github.com/iamdroppy/DomainKnock.git domain-knock
cd domain-knock
docker build . --tag domain-knock/latest
```

Running
```bash
docker run -it domain-knock/latest --help
```

## Usage

```  -v, --verbose           Verbose Level (0 = Info, 1 = Debug, 2 = Trace)
  -h, --hostname          Required.
  -o, --origin            Required. Start IP address.
  -d, --destination       Required. End IP address.
  -t, --timeout           Timeout in seconds (15 default)
  -p, --progress-delay    Delay (in seconds) between each progress scanning message. (30 default)
  --help                  Display this help screen.
  --version               Display version information.
  ```

  ## TODO

   - [ ] Multitasking scan
   - [ ] Custom ports (currently hardcoded as 80/443)
   - [ ] Custom user-agent
   - [ ] Custom headers (currently only the "required" and "most used" ones)
   - [ ] Better output
