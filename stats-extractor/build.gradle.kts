import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "1.5.20"

    id("com.github.ben-manes.versions") version "0.39.0"
    application
    idea
}

group = "audiomaze"
version = "1.0-SNAPSHOT"

repositories {
    mavenCentral()
}

dependencies {
    val kotlinVersion = "1.5.20"
    implementation(kotlin("stdlib-jdk8", version = kotlinVersion))

    val jacksonVersion = "2.12.3"
    implementation("com.fasterxml.jackson.core", "jackson-core", jacksonVersion)
    implementation("com.fasterxml.jackson.core", "jackson-annotations", jacksonVersion)
    implementation("com.fasterxml.jackson.core", "jackson-databind", jacksonVersion)
    implementation("com.fasterxml.jackson.module","jackson-module-kotlin", jacksonVersion)
}

application {
    mainClass.set("MainKt")
}

tasks.withType<KotlinCompile>() {
    kotlinOptions {
        jvmTarget = "16"
        languageVersion = "1.5"
    }
}

idea {
    module {
        isDownloadJavadoc = true
        isDownloadSources = true
    }
}
