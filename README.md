<p align="center">
  <img width="70%" src="res/logo.svg">
</p>

Blindness is a framework for you to build your frameworks without seeing the details.

# Table of Contents

 - [Overview](#overview)
 - [How to install](#how-to-install)
 - [Versions](#versions)

# Overview

Blidness will control your dependency injection system, your state management, your execution flow, your component oriented structure and your event system to allows you to concentrate where it really matters.

# How to install

```bash
dotnet new classlib # Create your library
dotnet add package Blindness # Install Blindness
```

# Versions

### Blindness v1.1.0 (Coming soon)

 - ![](https://img.shields.io/badge/update-blue) Comments and documentation improvements. 
 - ![](https://img.shields.io/badge/update-blue) Improvement in customization of Code Generation System.
  - ![](https://img.shields.io/badge/update-blue) Geral improvements in HotReload System.

### Blindness v1.0.0 (Coming soon)

 - ![](https://img.shields.io/badge/new-green) Code generation added.
 - ![](https://img.shields.io/badge/new-green) Concrete node generations system.
 - ![](https://img.shields.io/badge/new-green) HotReload system added.

### Blindness v0.5.0

 - ![](https://img.shields.io/badge/new-green) Event system added.

### Blindness v0.4.0

 - ![](https://img.shields.io/badge/new-green) Parallel and Async Node Actions System added.
 - ![](https://img.shields.io/badge/new-green) Special Nodes added.

### Blindness v0.3.0

 - ![](https://img.shields.io/badge/new-green) Basic flow control system.
 - ![](https://img.shields.io/badge/new-green) Verbose and exception system.
 - ![](https://img.shields.io/badge/bug%20solved-red) Many Bugs solved in field initialization.

### Blindness v0.2.0

 - ![](https://img.shields.io/badge/update-blue) Node creation systax changed to use interfaces.
 - ![](https://img.shields.io/badge/new-green) Dependency Injection System added.
 - ![](https://img.shields.io/badge/bug%20solved-red) Many Bugs solved in Binding System.

### Blindness v0.1.0

 - ![](https://img.shields.io/badge/new-green) Binding System added.

# TODO (For the developer team)

- Melhorar abstração do App para possibilitar customizações (crítico)
- Criar documentação da tecnologia
- Melhorar gerenciamento de memória e estruturas de dados internos
- Flexibilizar geração de código
- Melhorar validação 'needImplement'
- Permitir geração de nós concretos em profundidade (a discutir)
- Melhorar estrutura de Biding
  - Permitir binding functions (Bind |= x => operação(y))
  - Permitir vector binding (Bind |= x => list\[index\])