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

### Blindness v0.2.0

 - ![](https://img.shields.io/badge/update-blue) Node creation systax changed to use interfaces.
 - ![](https://img.shields.io/badge/new-green) Dependency Injection System added.
 - ![](https://img.shields.io/badge/bug%20solved-red) Many Bugs solved in Binding System.

### Blindness v0.1.0

 - ![](https://img.shields.io/badge/new-green) Binding System added.

# TODO (For the developer team)

- Melhorar divisão do código
- Melhorar tratamento de exceções
- Aplicar sistema verbose
- Flexibilizar controle do fluxo de execução
- Criar documentação da tecnologia
- Melhorar gerenciamento de memória e estruturas de dados internos
- Criar sistema de eventos usando o sistema de binding
- Melhorar estrutura de Biding
  - Permitir binding functions (Bind |= x => operação(y))
  - Permitir vector binding (Bind |= x => list\[index\])